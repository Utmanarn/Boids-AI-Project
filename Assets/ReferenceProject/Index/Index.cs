using UnityEngine;
using System;

/* Index */
public struct Index : IEquatable<Index>{
	private int i;
	private int j;
	
	public Index(int i, int j){
		this.i = i;
		this.j = j;		
	}

	public Index(Index index){
		i = index.i;
		j = index.j;		
	}

	public int I{
		get{
			return i;
		}
        set{
            i = value;
        }
	}
	
	public int J{
		get{
			return j;
		}
        set{
            j = value;
        }
	}

    public Index Left{
        get{
            return new Index(i - 1, j);
        }
    }

    public Index Right{
        get{
            return new Index(i + 1, j);
        }
    }

    public Index Down{
        get{
            return new Index(i, j - 1);
        }
    }

    public Index Up{
        get{
            return new Index(i, j + 1);
        }
    }

	private bool InternalEquals(Index index){
		return i == index.i && j == index.j;
	}

    public bool Equals(Index index){
		return InternalEquals(index);
	}

	public override bool Equals(object o){
		if(o == null || o.GetType() != GetType()){
			return false;
		}
		else{
			return InternalEquals((Index)o);	
		}
	}
	
	public override int GetHashCode(){
		return 0;
	}
	
	public override string ToString(){
		return string.Format("({0}, {1})", i, j);
	}
					
	public static bool operator==(Index index0, Index index1){
		return index0.InternalEquals(index1);
	}

	public static bool operator!=(Index index0, Index index1){
		return !index0.InternalEquals(index1);
	}
}



