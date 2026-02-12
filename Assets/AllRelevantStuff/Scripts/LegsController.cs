using UnityEngine;
using ExtensionMethods;

namespace CharacterMovement
{

    public interface ILegsController
    {

    }

    public class LegsController : MonoBehaviour
    {
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private SingleLegController _rightLeg;
        [SerializeField] private SingleLegController _leftLeg;

        [SerializeField] private Transform _rightFootTarget;
        [SerializeField] private Transform _leftFootTarget;

        private Vector2 _rightFootTargetPosition;
        private Vector2 _leftFootTargetPosition;

        private SingleLegController _footMovingLeg;


        void Update()
        {
            if (!ShouldMove())
                return;

            if(IsMovingFoot())
                return;

            ChooseFootMovingLeg();
            CalculateNewFootPositions();
            MoveLegs();

        }

        private bool ShouldMove()
        {
            return _movementController.CurrentSpeed != 0;
        }

        private void ChooseFootMovingLeg()
        {
            if(_footMovingLeg == null)
            {
                System.Random random = new System.Random();
                random.TossACoin();
            }
            else
            {
                _footMovingLeg = _footMovingLeg == _leftLeg ? _rightLeg : _leftLeg;
            }
        }

        private bool IsMovingFoot()
        {
            return _footMovingLeg != null;
        }

        private void CalculateNewFootPositions()
        {
            _rightFootTargetPosition = _rightFootTarget.position;
            _leftFootTargetPosition = _leftFootTarget.position;

            Vector2 footPositionOffset = CalculateFootOffset();

            if (_footMovingLeg == _rightLeg)
            {
                _rightFootTargetPosition +=  footPositionOffset;
            }
            else
            {
                _leftFootTargetPosition += footPositionOffset;
            }
        }

        private Vector2 CalculateFootOffset()
        {
            Vector2 footPositionOffset = _movementController.CurrentSpeed * _movementController.MovementDirection; // some kind of foot offset calculations

            return footPositionOffset;
        }

        private void MoveLegs()
        {
            _rightLeg.Step(_rightFootTargetPosition);
            _leftLeg.Step(_leftFootTargetPosition);
        }
    }
}
