using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace CPUSim
{
    static class Sim
    {
        public static TextBox[] RAM = new TextBox[256];     //This is one of the best ugly workarounds I've come up with
        public static TextBox[] reg = new TextBox[16];
        public static TextBox PC;
        public static bool isRunning = false;

        //These methods makes the variables above references to the TextBox Elements in the window
        //I am genuently sorry about this
        public static void SetRAM(Grid r)
        {
            int counter = 0;
            foreach (UIElement u in r.Children)
            {
                if (u.GetType() == typeof(WrapPanel))
                {
                    WrapPanel wp = (WrapPanel)u;
                    if (wp.Children.Count > 0)
                    {
                        if (wp.Children[0].GetType() == typeof(TextBox))
                        {
                            RAM[counter] = (TextBox)wp.Children[0];
                            counter++;
                        }
                    }
                }
            }
        }
        public static void Setreg(Grid r)
        {
            for (int i = 0; i < 16; i++)
            {
                reg[i] = (TextBox)((StackPanel)r.Children[i]).Children[1];
            }
            PC = (TextBox)((StackPanel)r.Children[16]).Children[1]; //I know this is hacky
        }

        public static bool Step()
        {
            int instr = int.Parse(PC.Text, System.Globalization.NumberStyles.HexNumber);
            char opc = RAM[instr].Text[0];

            string data;
            int to, from;
            byte num1, num2, res, arg;
            string temp, temp1, temp2;

            switch (opc)
            {
                case '0':   //0xxx No instruction
                    break;
                case '1':   //1RXY Copy content of RAM cell XY to register R
                    data = RAM[instr + 1].Text;
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    reg[to].Text = data;
                    break;
                case '2':   //2RXY Set value of register R to XY
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    reg[to].Text = RAM[instr + 1].Text;
                    break;
                case '3':   //3RXY Copy content of register R to RAM cell XY
                    to = int.Parse(RAM[instr + 1].Text, System.Globalization.NumberStyles.HexNumber);
                    from = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    RAM[to].Text = reg[from].Text;
                    break;
                case '4':   //4xRS Copy content of register R to register S
                    to = int.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    from = int.Parse(RAM[instr + 1].Text[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                    reg[to].Text = reg[from].Text;
                    break;
                case '5':   //5RST Add together register S and T as two's compliment, and store the result in register R
                    num1 = byte.Parse(RAM[instr + 1].Text[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num1 = byte.Parse(reg[num1].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    
                    res = (byte)(num1 + num2);
                    reg[to].Text = res.ToString("X");
                    break;
                case '6':   //5RST Add together register S and T as floating point, and store the result in register R
                    //Not implemented.
                    break;
                case '7':     //7RST Perform bitwise OR on register S and T, and store the result in register R
                    num1 = byte.Parse(RAM[instr + 1].Text[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num1 = byte.Parse(reg[num1].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    res = (byte)(num1 | num2);
                    reg[to].Text = res.ToString("X");
                    break;
                case '8':   //8RST Perform bitwise AND on register S and T, and store the result in register R
                    num1 = byte.Parse(RAM[instr + 1].Text[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num1 = byte.Parse(reg[num1].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    res = (byte)(num1 & num2);
                    reg[to].Text = res.ToString("X");
                    break;
                case '9':   //9RST Perform bitwise XOR on register S and T, and store the result in register R
                    num1 = byte.Parse(RAM[instr + 1].Text[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num1 = byte.Parse(reg[num1].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    to = int.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    res = (byte)(num1 ^ num2);
                    reg[to].Text = res.ToString("X");
                    break;
                case 'A':   //ARxX Cyclycally rotate register R, X steps from right to left
                    //This seems to only work once?!?!?
                    to = byte.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num1 = byte.Parse(reg[to].Text, System.Globalization.NumberStyles.HexNumber);
                    arg = byte.Parse(RAM[instr + 1].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    
                    //Convert.ToString(num1, 2));
                    temp = Convert.ToString(num1, 2);
                    temp1 = temp.Substring(0, arg);
                    temp2 = temp.Substring(arg);
                    temp = temp2 + temp1;
                    reg[to].Text = (Convert.ToByte(temp, 2)).ToString("X");
                    
                    break;
                case 'B':   //BRXY Jump to instruction in RAM cell XY if register R equals the content of register 0
                    num1 = byte.Parse(reg[0].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    //MessageBox.Show(num1.ToString() + " "+ num2.ToString() + " " + (num2==num1).ToString());
                    if (num1 == num2)
                    {
                        to = int.Parse(RAM[instr + 1].Text, System.Globalization.NumberStyles.HexNumber);
                        PC.Text = (to - 2).ToString("X");
                    }
                    break;
                case 'C':   //Cxxx Stops the program
                    //Not implementet
                    return true;
                case 'D':   //BRXY Jump to instruction in RAM cell XY if register R is larger than register 0
                    sbyte snum1 = sbyte.Parse(reg[0].Text, System.Globalization.NumberStyles.HexNumber);
                    num2 = byte.Parse(RAM[instr].Text[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    sbyte snum2= sbyte.Parse(reg[num2].Text, System.Globalization.NumberStyles.HexNumber);
                    if (snum1 < snum2)
                    {
                        to = int.Parse(RAM[instr + 1].Text, System.Globalization.NumberStyles.HexNumber);
                        PC.Text = (to - 2).ToString("X");
                    }
                    break;
                case 'E':   //ERXY Set pixel XY to colour in register R
                    //Not implemented
                    break;
                case 'F':    //FxxR Store currently pressed keyboard key as an ASCII character in register R
                    //not implemented

                    break;
                default:
                    MessageBox.Show("Invalid Opcode: " + opc);
                    break;
            }
            instr = int.Parse(PC.Text, System.Globalization.NumberStyles.HexNumber);
            if (instr > 255-2)
            {
                instr -= 256;
            }
            PC.Text = (instr + 2).ToString("X");
            return false;
        }

        public static bool Cycle()
        {
            if (isRunning)
            {
                return Step();
            }
            return false;
        }
    }
}
