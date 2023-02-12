using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class main : MonoBehaviour{
    // Step audio clips to be played
    public AudioClip step_1, step_2, step_3, step_4;

    // Number for raycast per layer. Three layers: central one (horizontal), 2 diagonal
    public static int tot_ray = 16;
    // Angle used for the three raycast lines
    float[] pitch = {-0.45f, 0.0f, 0.45f};
    // Field Of View, used to define how broad the raycast will be generated
    float fov = 70;
    // Current raycast angle, used to position the raycast in the right spot 
    float ray_angle;

    // Array used to store created raycasts
    Ray[] rays = new Ray[tot_ray * 3];

    // Magnitude of step value between different audio clips
    float stepDelta = 0.4f;
    // Minimum distance from which the audio feedback starts
    float DistanceThreshold = 1.6f;
    // Minimum distance of a raycast hit from the closest obstacle
    float minDistance;

    // Variable used to store the closest raycast with its assosiated angle
    ray_sound target = new ray_sound();

    // Start Function (First function that will be executed when the program starts) [Executed only once]
    void Start(){
        // Get audiosource component from gameobject. Used to play the proximity feedback sounds
        target.audio = GetComponent<AudioSource>();
    }

    // Second function that gets executed [Executed repeatedly]
    void Update(){
        RaycastHit hit;
        //Update the minDistance with the DistanceThreshold
        minDistance = DistanceThreshold;
        // Creating the three raycasts lines, one line at the time, each with a particular angle defined in pitch array
        for (int j = 0; j < pitch.Length; j++){
            // Assign to the first raycast the left most orientation
            ray_angle = - fov / 2;
            for (int i = 0; i < tot_ray; i++){
                Vector3 dir;
                // If pitch angle is not 0, it means that the current raycast is contained in one of the two diagonal sets
                if (pitch[j] > 0 || pitch[j] < 0){
                    dir = Quaternion.AngleAxis(ray_angle, new Vector3(0, 1, 0)) * (new Vector3(0, pitch[j], 0) + transform.forward) * DistanceThreshold;
                    Debug.DrawRay(transform.position, dir, Color.blue);
                }else{ // Otherwise it is contained in the horizontal one
                    dir = Quaternion.AngleAxis(ray_angle, new Vector3(0, 1, 0)) * transform.forward * DistanceThreshold;
                    Debug.DrawRay(transform.position, dir, Color.green);
                }
                // Store each generated raycast
                rays[i] = new Ray(transform.position, dir);

                // Checks if raycast hits something
                if (Physics.Raycast(rays[i], out hit)){
                    // If the current raycast hits a gameobject with tag "obstacle" check if it closer than the current minimum
                    if (hit.collider.tag == "obstacle"){
                        // If the raycast hit distance is less than the current minDistance, update minDistance and store raycast inside target class
                        if (hit.distance < minDistance){
                            minDistance = hit.distance;
                            //save the ray and its angle in a function
                            target.index = i;
                            target.ray = rays[i];
                            target.angle = ray_angle;
                        }
                    }
                }
                // Update val by adding angle step
                ray_angle = ray_angle + angle_step(tot_ray);
            }
        }

        // Draw red the raycast that is closest to obstacle
        Debug.DrawRay(target.ray.origin, target.ray.direction * DistanceThreshold, Color.red);
        // Rimap value of angle with the range of PanStereo
        set_RimapPanstereoValue(target,fov);

        // Check if the lowest distance to obstacle is less than DistanceThreshold, if yes it means that it is close enough to start feedback sound
        if (minDistance < DistanceThreshold){
            Sound_step();
            //Debug.Log("dis: " + minDistance);
        }else{
            // Else the user is far enough, so can stop playing the audio clip
            target.audio.Stop();
        }
    }

    void Sound_step(){
        // Setting the step sound based on the distance from obstacle
        if (minDistance < DistanceThreshold - 3 * stepDelta)
            target.audio.clip = step_4;                                       // Closest one to obstacle
        else if (minDistance < DistanceThreshold - 2 * stepDelta)
            target.audio.clip = step_3;
        else if (minDistance < DistanceThreshold - stepDelta)
            target.audio.clip = step_2;
        else target.audio.clip = step_1;                                      // Furthest one to obstacle

        // Check if audo is already playing, if not play audo clip
        if (!target.audio.isPlaying){
            target.audio.Play(0);
        }
    }

    void set_RimapPanstereoValue(ray_sound r,float fov){
    // Reset value of the angle to set of the PanStereoValue
        float y = map(r.angle, -fov/2, +fov/2, -1, +1);
        r.audio.panStereo = y;
    }

    float map(float x, float in_min, float in_max, float out_min, float out_max){
    // Mapping the value from input range to another output range
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public float angle_step(int tot){
    // Set each angle between the rays of the Field_Of_View (FOV)
        float tot_uno = (tot - 1);
        float step = fov / tot_uno;
        return step;
    }

    class ray_sound{
    // New variable containing the value of Ray, its AudioSource, its angle and its index
        public Ray ray;
        public AudioSource audio;
        public float angle;
        public int index;
    }
}