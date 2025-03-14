using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FluidShaderWarp : MonoBehaviour
{
    [SerializeField] private Material m_shader;
    [SerializeField] private int m_iterator;
    [SerializeField] private Vector4 m_warpFactor;
    private float m_value;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartWarpFluid());
    }

    // Update is called once per frame
    void Update()
    {




    }

    private IEnumerator StartWarpFluid()
    {
        if (m_iterator == 100)
        {
            m_iterator = 1;
        }
        else
        {
            if (m_iterator < 50)
            {
                yield return new WaitForSeconds(0.1f);
                IterateUp();
            }

            if (m_iterator >= 50)
            {
                yield return new WaitForSeconds(0.1f);
                IterateDown();
            }
        }
    }

    private void IterateUp()
    {
        for (int i = 0; i < 50; i++)
        {
            var vec4x = m_shader.GetVector("_WarpModifier2").x + m_warpFactor.x;
            var vec4y = m_shader.GetVector("_WarpModifier2").y + m_warpFactor.y;
            m_shader.SetVector("_WarpModifier2", new Vector4(vec4x, vec4y));
            m_iterator = i;
            Debug.Log("VecShaderX: " + m_shader.GetVector("_WarpModifier2").x + "VecShaderY: " + m_shader.GetVector("_WarpModifier2").y + m_warpFactor.y + " || " + "Iterator: " + m_iterator);
        }
    }

    private void IterateDown()
    {
        for (int i = 0; i >= 50; i--)
        {
            var vec4x = m_shader.GetVector("_WarpModifier2").x - m_warpFactor.x;
            var vec4y = m_shader.GetVector("_WarpModifier2").y - m_warpFactor.y;
            m_shader.SetVector("_WarpModifier2", new Vector4(vec4x, vec4y));
            m_iterator = i;
            Debug.Log("VecShaderX: " + m_shader.GetVector("_WarpModifier2").x + "VecShaderY: " + m_shader.GetVector("_WarpModifier2").y + m_warpFactor.y + " || " + "Iterator: " + m_iterator);
        }
    }


}
