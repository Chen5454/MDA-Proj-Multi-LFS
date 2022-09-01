using UnityEngine;

public class LightFlashing : MonoBehaviour
{
	public float time = 10f;

    [Space]
    public float minIntensity = -5f;
	public float maxIntensity = 5f;

    [Space]
    public float minTint = 0.1f;
    public float maxTint = 1f;

    [Space]
    public bool useSmooth = false;
	public float smoothTime = 5f;

    private float m_percent;
    private float m_lightAmount;
    private bool m_negative;

    [Space]
    public GameObject glowGO;

	public Color color = Color.white;
	
	private Light m_Light;
	private Material m_Material;


	private void Start() 
	{
		m_Light = GetComponent<Light>();
		m_Material = glowGO.GetComponent<Renderer>().material;
		
		if(useSmooth == false && m_Light != null)
		{
			InvokeRepeating("OneLightChange", time, time);
            color = m_Material.GetColor("_TintColor");
        }
    }

	private void Update() 
	{
        if (useSmooth && m_Light != null)
        {
            m_lightAmount = Mathf.PingPong(Time.time * smoothTime, (maxIntensity - minIntensity)) + minIntensity;
            m_Light.intensity = m_lightAmount;

            CalculateMaterial();
        }
    }


    private void OneLightChange()
    {
        m_lightAmount = Mathf.PingPong(Time.time * time, (maxIntensity - minIntensity)) + minIntensity;
        m_Light.intensity = m_lightAmount;

        CalculateMaterial();
    }

    private void CalculateMaterial()
    {
        m_percent = Mathf.Abs(m_lightAmount) / (maxIntensity - minIntensity);
        m_negative = (m_lightAmount < 0);

        if (!m_negative)
        {
            color.a = (m_percent * (maxTint - minTint)) + minTint;
            m_Material.SetColor("_TintColor", color);
        }
    }

}