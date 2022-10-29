using BehaviourGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviourGraph.Runtime
{

    [RequireComponent(typeof(BlackboardVariable))]
    [AddComponentMenu("Behaviour Graph/Behaviour Owner")]
    public class BehaviourOwner : MonoBehaviour, IBehaviourOwner
    {
        public enum UpdateMode
        {
            Update,
            LateUpdate,
            FixedUpdate,
            CustomInterval
        }

        [SerializeField]
        private BehaviourAsset behaviourAsset;

        [SerializeField]
        private UpdateMode updateMode = UpdateMode.Update;

        private IBlackboard blackboard;
        public IBlackboard Blackboard
        {
            get => blackboard ??= GetComponent<BlackboardVariable>();
        }

        private NodeState state = NodeState.Running;
        
        private void Start()
        {
            behaviourAsset.DataSource.AllNodes.ForEach(node =>
            {
                (node as ITask).Initialize(this);
            });
        }

        private void Update()
        {
            Evaluate(UpdateMode.Update);
        }

        private void FixedUpdate()
        {
            Evaluate(UpdateMode.FixedUpdate);
        }

        private void LateUpdate()
        {
            Evaluate(UpdateMode.LateUpdate);
        }

        private void Evaluate(UpdateMode evaluateUpdateMode)
        {
            if (this.updateMode != evaluateUpdateMode) return;

            if (behaviourAsset.DataSource.RootTask != null && state == NodeState.Running)
            {
                state = behaviourAsset.DataSource.RootTask.Evaluate(Blackboard);
            }
        }

        public IBehaviour GetBehaviour()
        {
            if (behaviourAsset != null)
            {
                behaviourAsset.BehaviourOwner = this;
                return behaviourAsset;
            }

            return null;
        }
    }
}
