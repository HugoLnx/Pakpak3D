using System;
using System.Collections;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class HuntInstructorService : MonoBehaviour
    {
        [SerializeField] private HuntInstruction _instruction = HuntInstruction.Attack;
        [SerializeField] private float _attackDuration = 10f;
        [SerializeField] private float _scatterDuration = 5f;
        public HuntInstruction Instruction => _instruction;
        public event Action<HuntInstruction> OnInstructionChanged;

        private void Start()
        {
            StartCoroutine(SwitchInstructions());
            StartCoroutine(ReinforceInstruction());
        }

        public void SetAttackDuration(float duration)
        {
            Debug.Log($"Attack duration set to {duration}");
            _attackDuration = duration;
        }

        public void SetScatterDuration(float duration)
        {
            Debug.Log($"Scatter duration set to {duration}");
            _scatterDuration = duration;
        }

        public void SpreadInstruction(HuntInstruction instruction)
        {
            // Debug.Log($"Instruction changed to {instruction}");
            _instruction = instruction;
            OnInstructionChanged?.Invoke(instruction);
        }

        private IEnumerator ReinforceInstruction()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                SpreadInstruction(_instruction);
            }
        }

        private IEnumerator SwitchInstructions()
        {
            while (true)
            {
                HuntInstruction newInstruction = _instruction == HuntInstruction.Attack ? HuntInstruction.Scatter : HuntInstruction.Attack;
                SpreadInstruction(newInstruction);
                if (newInstruction == HuntInstruction.Attack)
                {
                    yield return new WaitForSeconds(_attackDuration);
                }
                else
                {
                    yield return new WaitForSeconds(_scatterDuration);
                }
            }
        }
    }
}
