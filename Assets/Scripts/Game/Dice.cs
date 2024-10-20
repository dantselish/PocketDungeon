using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

public class Dice : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private List<DiceSide> Sides;

    private bool _isThrown;
    private bool _isCalculated;

    public event Action<int> DiceStopped;


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!_isCalculated && _isThrown && Rigidbody.velocity.magnitude == 0f && Rigidbody.angularVelocity.magnitude == 0f)
        {
            CalculateResult();
        }
    }

    public void Throw()
    {
        gameObject.SetActive(true);
        Rigidbody.isKinematic = false;

        Vector3 randomRotation = new Vector3(Random.Range(0f, 365f), Random.Range(0f, 365f), Random.Range(0f, 365f));
        transform.rotation = Quaternion.Euler(randomRotation);

        Vector3 randomForceDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        float randomForcePower = Random.Range(0f, 10f);
        Rigidbody.AddForce(randomForceDirection * randomForcePower, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        float randomTorquePower = Random.Range(0f, 10f);
        Rigidbody.AddTorque(randomTorque * randomTorquePower, ForceMode.Impulse);

        Invoke(nameof(SetIsThrown), 0.1f);
    }

    private void SetIsThrown()
    {
        _isThrown = true;
    }

    private void CalculateResult()
    {
        DiceSide highestSide = Sides.First();
        foreach (DiceSide side in Sides)
        {
            if (side.transform.position.y > highestSide.transform.position.y)
            {
                highestSide = side;
            }
        }

        _isCalculated = true;
        DiceStopped?.Invoke(highestSide.SideNumber);
    }
}
