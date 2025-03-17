using System.Collections;
using UnityEngine;
using Veridium.Animation;
using Veridium.Interaction;

namespace Veridium.Modules.TutorialModule {
    public class Await_GrabModule : Await_Grab
    {
        public string moduleName;
        public FindModulesInBuild findModulesInBuild;

        protected override void Start()
        {
            // dont start yet, wait for first frame so that the modules are loaded
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            findModulesInBuild.modules.ForEach(module => {
                if(module.displayName == moduleName){
                    grabInteractable = module.GetComponent<HandDistanceGrabbable>();
                }
            });

            base.Start();
        }
    }
}
