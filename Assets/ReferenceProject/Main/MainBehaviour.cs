using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/* MainBehaviour */
public class MainBehaviour : MonoBehaviour {


    /* Properties */
    [System.Serializable]
    private abstract class Properties{
        public abstract void OnValidate(MainBehaviour main);
        public abstract void Awake(MainBehaviour main);        
    }

    /* EnvironmentProperties */
    [System.Serializable]
    private class EnvironmentProperties : Properties{

        /* EnvironmentScene */
        [System.Serializable]
        private class EnvironmentScene{
            [SerializeField] private string name;
            [SerializeField] private string path;

            public string Name{
                get{
                    return name;    
                }    
            }

            public string Path{
                get{
                    return path;    
                }    
            }

            public bool ValidPath{
                get{
                    return 0 <= SceneUtility.GetBuildIndexByScenePath(path);    
                }
            }

            public EnvironmentScene(string name, string path){
                this.name = name;
                this.path = path;
            }
        }

        [SerializeField] private Dropdown environmentDropdown = null;
        [SerializeField] private List<EnvironmentScene> environmentScenes = new List<EnvironmentScene>();
        
        public int CurrentEnvironmentIndex{
            get{
                return environmentDropdown.value;    
            }    
        }

        public string GetScenePath(int environmentIndex){
            return environmentScenes[environmentIndex].Path;
        }

        public int GetSceneBuildIndex(int environmentIndex){
            return SceneUtility.GetBuildIndexByScenePath(GetScenePath(environmentIndex));
        }

        public AsyncOperation LoadSceneAsynch(int environmentIndex){
            int buildIndex = GetSceneBuildIndex(environmentIndex);
            return SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);   
        }

        public AsyncOperation UnloadSceneAsynch(int environmentIndex){
            int buildIndex = GetSceneBuildIndex(environmentIndex);
            return SceneManager.UnloadSceneAsync(buildIndex); 
        }

        public override void OnValidate(MainBehaviour main){
            if(environmentDropdown == null){
                Debug.LogWarning("There is no environment dropdown.", main);
            }
            if(environmentScenes.Count == 0){
                Debug.LogWarning("There are no environment scenes.", main);
            }
        }

        private void ValidateEnvironmentScenePaths(MainBehaviour main){
            foreach(EnvironmentScene environmentScene in environmentScenes){
                if(!environmentScene.ValidPath){
                    Debug.LogWarning("Invalid environment scene path: " + environmentScene.Path, main);
                }
            }
        }

        public override void Awake(MainBehaviour main){
            ValidateEnvironmentScenePaths(main);
            environmentDropdown.ClearOptions();
            environmentDropdown.AddOptions(environmentScenes.Select(environmentScene => environmentScene.Name).ToList());
            environmentDropdown.RefreshShownValue();
            environmentDropdown.onValueChanged.AddListener(main.OnSelectEnvironment);   
        }
    }

    /* State */
    private abstract class State{
        private State parentState;
        private MainBehaviour main;

        public State ParentState{
            get{
                return parentState;    
            }    
        }

        public MainBehaviour Main{
            get{
                return main;    
            }    
        }

        public State(State parentState, MainBehaviour main){
            this.parentState = parentState;
            this.main = main;
        }

        public EnvironmentProperties EnvironmentProperties{
            get{
                return main.environmentProperties;    
            }    
        }

        public abstract void Transit(State fromState, State toState);
        public abstract void Enter();
        public abstract void Exit();
        public abstract bool Update();
        public abstract bool OnSelectEnvironment(int environmentIndex);
    }

    /* LeafState */
    private abstract class LeafState : State{
    
        public LeafState(State parentState, MainBehaviour main) : base(parentState, main){            
        }          
        
        public override void Transit(State fromState, State toState){
            ParentState.Transit(fromState, toState);    
        }

        public override void Enter(){
        }

        public override void Exit(){            
        }

        public override bool Update(){
            return false;    
        }

        public override bool OnSelectEnvironment(int environmentIndex){
            return false;    
        }
    }

    /* EnvironmentState */
    private class EnvironmentState : State{
        
        /* IdleState */
        private class IdleState : LeafState{
            private int environmentIndex;

            public IdleState(State parentState, MainBehaviour main, int environmentIndex) : base(parentState, main){
                this.environmentIndex = environmentIndex;    
            }

            public override bool OnSelectEnvironment(int environmentIndex){
                Transit(this, new ReplaceState(ParentState, Main, this.environmentIndex, environmentIndex));
                return true;
            }
        }

        /* ReplaceState */
        private class ReplaceState : LeafState{
            private int unloadEnvironmentIndex;
            private int loadEnvironmentIndex;
            private AsyncOperation unloadOperation;
            private AsyncOperation loadOperation;

            public ReplaceState(State parentState, MainBehaviour main, int unloadEnvironmentIndex, int loadEnvironmentIndex) : base(parentState, main){
                this.unloadEnvironmentIndex = unloadEnvironmentIndex;
                this.loadEnvironmentIndex = loadEnvironmentIndex;
                unloadOperation = null;
                loadOperation = null;
            }

            public override void Enter(){
                if(0 <= unloadEnvironmentIndex){
                    unloadOperation = EnvironmentProperties.UnloadSceneAsynch(unloadEnvironmentIndex);
                }
                loadOperation = EnvironmentProperties.LoadSceneAsynch(loadEnvironmentIndex);
            }

            public override bool Update(){
                bool unloadingDone = unloadOperation == null || unloadOperation.isDone;
                bool loadingDone = loadOperation.isDone;
                if(unloadingDone && loadingDone){
                    Transit(this, new IdleState(ParentState, Main, loadEnvironmentIndex));
                }
                return true;
            }
        }

        private State childState;

        public EnvironmentState(MainBehaviour main) : base(null, main){
            childState = new ReplaceState(this, main, -1, 0);
        }

        public override void Transit(State fromState, State toState){
            if(childState == fromState){
                childState.Exit();
                childState = toState;
                childState.Enter();
            }
            else{
                ParentState.Transit(fromState, toState);
            }
        }

        public override void Enter(){
            childState.Enter();    
        }

        public override void Exit(){
            childState.Exit();
        }

        public override bool Update(){
            return childState.Update();    
        }

        public override bool OnSelectEnvironment(int environmentIndex){
            return childState.OnSelectEnvironment(environmentIndex);    
        }
    }

    [SerializeField] private EnvironmentProperties environmentProperties = new EnvironmentProperties();
    private State state = null;

    public void OnValidate(){
        environmentProperties.OnValidate(this);    
    }

    public void Awake(){
        environmentProperties.Awake(this);
        state = new EnvironmentState(this);
        state.Enter();
    }

    public void Update(){
       state.Update();
    }

    public void OnSelectEnvironment(int environmentIndex){
        state.OnSelectEnvironment(environmentIndex);
    }
}
