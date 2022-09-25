using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEV_Target : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private Material m_Red;
    [SerializeField] private Material m_Green;

    private float m_TurnTime = 3;

    IEnumerator TurnBack()
    {
        yield return new WaitForSeconds(m_TurnTime);
        ChangeMat(m_Red);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ChangeMat(m_Green);
        StartCoroutine(TurnBack());
    }

    private void ChangeMat(Material mat)
    {
        m_Renderer.material = mat;
    }
}
