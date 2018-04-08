using System;

namespace TestTask_Nival
{
    public class Calculation
    {
        private readonly string uid;
        private readonly Operand operand;
        private readonly int mod;

        public Calculation(string uid, Operand operand, int mod)
        {
            this.uid = uid;
            this.operand = operand;
            this.mod = mod;
        }

        public string UID => uid;
        public Operand Operand => operand;
        public int Mod => mod;

        public int Process(int initialValue)
        {
            switch (operand)
            {
                case Operand.Add: return initialValue + mod;
                case Operand.Subtract: return initialValue - mod;
                case Operand.Multiply: return initialValue * mod;
                case Operand.Divide: return initialValue / mod;
                default: throw new ArgumentException("Invalid operand");
            }
        }
    }

    public enum Operand { Add, Subtract, Multiply, Divide }
}
