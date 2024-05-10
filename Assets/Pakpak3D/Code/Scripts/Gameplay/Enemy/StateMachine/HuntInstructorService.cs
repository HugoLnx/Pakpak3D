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
        public HuntInstruction Instruction => _instruction;
        public event Action<HuntInstruction> OnInstructionChanged;

        public void SpreadInstruction(HuntInstruction instruction)
        {
            Debug.Log($"Instruction changed to {instruction}");
            _instruction = instruction;
            OnInstructionChanged?.Invoke(instruction);
        }

        private void Start()
        {
            StartCoroutine(SwitchInstructions());
        }

        private IEnumerator SwitchInstructions()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                HuntInstruction newInstruction = _instruction == HuntInstruction.Attack ? HuntInstruction.Scatter : HuntInstruction.Attack;
                SpreadInstruction(newInstruction);
            }
        }
    }
}
