using UnityEngine;
using System.Collections;

/* Timer */
public class Timer{
	private float duration;
	private float time;
    private float tolerance;

	public Timer(float duration = 0.0f, float time = 0.0f, float tolerance = 0.001f){
		this.duration = duration;
		this.time = time;
        this.tolerance = tolerance;
	}

    public bool Valid{
        get{
            return tolerance < duration;
        }
    }

    private bool ExpiredImp{
        get{
            return duration < time;
        }
    }

    public bool Expired{
        get{
            return Valid ? ExpiredImp : true;
        }
    }

    public float Duration{
        get{
            return duration;
        }
        set{
            duration = value;
        }
    }

    public float Time{
        get{
            return time;
        }
        set{
            time = value;
        }
    }

    private float RatioImp{
        get{
            return time / duration;
        }
    }

    public float Ratio{
        get{
            return Valid ? RatioImp : 0.0f;
        }
    }

    public float Fraction{
        get{
            return Valid ? Mathf.Clamp01(RatioImp) : 0.0f;
        }
    }

    public void Set(float duration = 0.0f, float time = 0.0f, float tolerance = 0.001f){
        this.duration = duration;
        this.time = time;
        this.tolerance = tolerance;
    }
}
