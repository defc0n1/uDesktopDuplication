﻿using UnityEngine;

namespace uDesktopDuplication
{

[AddComponentMenu("uDesktopDuplication/Texture")] 
public class Texture : MonoBehaviour
{
    private Monitor monitor_;
    public Monitor monitor 
    { 
        get { return monitor_; }
        set 
        { 
            monitor_ = value;
            material = GetComponent<Renderer>().material; // clone
            material.mainTexture = monitor_.texture;
            material.SetFloat("_Width", transform.localScale.x);
        }
    }

    public int monitorId
    { 
        get { return monitor.id; }
        set { monitor = Manager.GetMonitor(value); }
    }

    [Header("Invert UVs")]
    public bool invertX = false;
    public bool invertY = false;

    [Header("Clip")]
    public bool useClip = false;
    public Vector2 clipPos = Vector2.zero;
    public Vector2 clipScale = new Vector2(0.2f, 0.2f);

    public enum MeshForwardDirection
    {
        Y = 0,
        Z = 1,
    }

    public bool bend
    {
        get 
        {
            return material.GetInt("_Bend") != 0;
        }
        set 
        {
            if (value) {
                material.EnableKeyword("BEND_ON");
            } else {
                material.DisableKeyword("BEND_ON");
            }
        }
    }

    public MeshForwardDirection meshForwardDirection
    {
        get 
        {
            return (MeshForwardDirection)material.GetInt("_Forward");
        }
        set 
        {
            switch (value) {
                case MeshForwardDirection.Y:
                    material.SetInt("_Forward", 0);
                    material.EnableKeyword("_BEND_Y");
                    material.DisableKeyword("_BEND_Z");
                    break;
                case MeshForwardDirection.Z:
                    material.SetInt("_Forward", 1);
                    material.DisableKeyword("_BEND_Y");
                    material.EnableKeyword("_BEND_Z");
                    break;
            }
        }
    }

    public float radius
    {
        get { return material.GetFloat("_Radius"); }
        set { material.SetFloat("_Radius", value); }
    }

    public float width
    {
        get { return material.GetFloat("_Width"); }
        set { material.SetFloat("_Width", value); }
    }

    public float thickness
    {
        get { return material.GetFloat("_Thickness"); }
        set { material.SetFloat("_Thickness", value); }
    }

    public Material material
    {
        get;
        private set;
    }

    void Awake()
    {
        if (!GetComponent<Cursor>())
        {
            gameObject.AddComponent<Cursor>();
        }
    }

    void OnEnable()
    {
        if (monitor == null) {
            monitor = Manager.primary;
        }
    }

    void OnDisable()
    {
        Destroy(material);
    }

    void Update()
    {
        monitor.shouldBeUpdated = true;
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        Invert();
        Rotate();
        Clip();
    }

    void Invert()
    {
        if (invertX) {
            material.EnableKeyword("INVERT_X");
        } else {
            material.DisableKeyword("INVERT_X");
        }

        if (invertY) {
            material.EnableKeyword("INVERT_Y");
        } else {
            material.DisableKeyword("INVERT_Y");
        }
    }

    void Rotate()
    {
        switch (monitor.rotation)
        {
            case MonitorRotation.Identity:
                material.DisableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate90:
                material.EnableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate180:
                material.DisableKeyword("ROTATE90");
                material.EnableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate270:
                material.DisableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.EnableKeyword("ROTATE270");
                break;
            default:
                break;
        }
    }

    void Clip()
    {
        if (useClip) {
            material.EnableKeyword("USE_CLIP");
            material.SetVector("_ClipPositionScale", new Vector4(clipPos.x, clipPos.y, clipScale.x, clipScale.y));
        } else {
            material.DisableKeyword("USE_CLIP");
        }
    }
}

}