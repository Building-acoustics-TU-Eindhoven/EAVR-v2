/**
 * @author [Alessia Milo]
 * @email [a.milo@tue.nl]
 * @create date 2021-04-27 15:04:30
 * @modify date 2021-04-27 15:04:30
 * @desc [Create current observation and observation lists]
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AssessmentData{
    
    public string assessment_name;
    //public int value;
    public Observation currentObservation = new Observation();
    //public List<Effect> effects = new List<Effect>();

    public List<Observation> observations = new List<Observation>();

}