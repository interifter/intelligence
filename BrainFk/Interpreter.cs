using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFk {
    public class Interpreter {
        public static byte[] Interpret(string code, int? maxOutputLength = null, params byte[] input) {
            int inputPointer = 0;
            int LENGTH = 8;

            StringBuilder debug = new StringBuilder();

            byte[] mem = new byte[LENGTH];

            for (int ip = 0; ip < input.Length; ip++) {
                mem[ip] = input[ip];

                while (ip >= mem.Length) {
                    LENGTH += 8;
                    Array.Resize(ref mem, LENGTH);
                }
            }

            int dataPointer = 0;
            int i = 0;
            int outputLength = 0;
            for (i = 0; i < code.Length; i++) {
                debug.Append(code[i]);
                while (dataPointer >= LENGTH) {
                    LENGTH += 8;
                    Array.Resize(ref mem, LENGTH);
                }
                if (code[i] == '>') {
                    dataPointer = (dataPointer == LENGTH - 1) ? 0 : dataPointer + 1;
                }
                else if (code[i] == '<') {
                    dataPointer = (dataPointer == 0) ? LENGTH - 1 : dataPointer - 1;
                }
                else if (code[i] == '+') {
                    mem[dataPointer]++;
                }
                else if (code[i] == '-') {
                    mem[dataPointer]--;
                }
                else if (code[i] == '.') {
                    if (maxOutputLength != null & outputLength++ < maxOutputLength) {
                        Debug.Write((char)mem[dataPointer]);
                    }
                    else if (maxOutputLength == null) {

                    }
                    else {
                        throw new Exception(" :-) ");
                    }
                }
                else if (code[i] == ',') {
                    mem[dataPointer] = input[inputPointer++];
                }
                else if (code[i] == '[') {
                    if (mem[dataPointer] == 0) {
                        int loop = 1;
                        while (loop > 0) {
                            i++;
                            char c = code[i];
                            if (c == '[') {
                                loop++;
                            }
                            else
                            if (c == ']') {
                                loop--;
                            }
                        }
                    }
                }
                else if (code[i] == ']') {
                    int loop = 1;
                    while (loop > 0) {
                        i--;
                        char c = code[i];
                        if (c == '[') {
                            loop--;
                        }
                        else
                        if (c == ']') {
                            loop++;
                        }
                    }
                    i--;
                }
            }

            return mem;
        }
    }
}
