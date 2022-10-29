using BehaviourGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviourGraph.Runtime
{

    [RequireComponent(typeof(BlackboardVariable))]
    [AddComponentMenu("Behavior Graph/Behavior Owner")]
    public class BehaviorOwner : MonoBehaviour, IBehaviourOwner
    {
        public enum UpdateMode
        {
            Update,
            LateUpdate,
            FixedUpdate,
            CustomInterval
        }

        [SerializeField]
        private BehaviorAsset behaviorAsset;

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
            behaviorAsset.DataSource.AllNodes.ForEach(node =>
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

            if (behaviorAsset.DataSource.RootTask != null && state == NodeState.Running)
            {
                state = behaviorAsset.DataSource.RootTask.Evaluate(Blackboard);
            }
        }

        public void SetBehavior(BehaviorAsset behaviorAsset)
        {
            this.behaviorAsset = behaviorAsset;
        }

        public IBehaviour GetBehavior()
        {
            if (behaviorAsset != null)
            {
                behaviorAsset.BehaviourOwner = this;
                return behaviorAsset;
            }

            return null;
        }
    }
}
