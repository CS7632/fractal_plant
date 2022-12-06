using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PenToolController : MonoBehaviour
{
    [Header("Lines")]
    [SerializeField] private GameObject linePrefab;

    [Header("L-System Customization")] 
    [SerializeField] private float baseAngle = 15.0f;
    [SerializeField] private float maxAngleVariance = 3.0f;
    [SerializeField] private float segmentScalar = 0.15f;
    [SerializeField] private int numEpochs = 100;
    [SerializeField] private float xfSplitPoint = 0.5f;
    

    private string GeneratePlant()
    {
        const string start = "x";
        var current = start;

        for (var i = 0; i < numEpochs; i++)
        {
            var replaceAt = -1;
            var v = "";
            
            while (replaceAt < 0)
            {
                var repl = (Random.Range(0, 1.0f) < xfSplitPoint) ? 'x' : 'f';
                v = (repl == 'x') ? "f+[[x]-x]-f[-fx]+x" : "ff";
                
                replaceAt = Random.Range(0, current.Length);
                if (current[replaceAt] != repl)
                    replaceAt = current.IndexOf(repl, replaceAt);
            }
            
            var beginning = current[..replaceAt];
            var end = current.Substring(replaceAt + 1, current.Length - replaceAt - 1);

            current = beginning + v + end;
        }

        return current;
    }

    private float Noise()
    {
        var noise = maxAngleVariance * Random.Range(-1.0f, 1.0f);
        return noise;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        foreach(Transform child in transform)
            GameObject.Destroy(child.gameObject);

        var plant = GeneratePlant();
        var pos = GetMousePosition();
        var p = new Vector3(pos.x, pos.y, -1.0f);

        var lastD = new Vector3(1.0f, 0.0f, 0.0f);
        var undoStack = new Stack<Tuple<Vector3, Vector3>>();

        // var lineController = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, this.transform)
        //     .GetComponent<LineController>();
        
        foreach (var c in plant)
        {
            lastD = lastD.normalized;

            switch (c)
            {
                case 'x':
                    break;
                case 'f':
                    var lineController = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, this.transform)
                        .GetComponent<LineController>();
                    var tgt = p + lastD * segmentScalar;
                    lineController.AddPoint(p);
                    lineController.AddPoint(tgt);
                    p = tgt;
                    break;
                case '[':
                    undoStack.Push(new Tuple<Vector3, Vector3>(p, lastD));
                    break;
                case ']':
                    var oldState = undoStack.Pop();
                    p = oldState.Item1;
                    lastD = oldState.Item2;
                    
                    lineController = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, this.transform)
                        .GetComponent<LineController>();
                    break;
                case '+':
                    lastD = Quaternion.Euler(0, 0, baseAngle + Noise()) * lastD;
                    break;
                case '-':
                    lastD = Quaternion.Euler(0, 0, -baseAngle + Noise()) * lastD;
                    break;
            }
        }
    }

    private static Vector3 GetMousePosition()
    {
        var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePos.z = 0;
        return worldMousePos;
    }
}
