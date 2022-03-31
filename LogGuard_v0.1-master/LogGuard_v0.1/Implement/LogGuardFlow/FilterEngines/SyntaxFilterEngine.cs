using LogGuard_v0._1.Base.LogGuardFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines
{
    public class SyntaxFilterEngine : NormalFilterEngine
    {
        private string[] PostFix { get; set; } = new string[0];
        private string[] PartsSource { get; set; }
        private bool _isVailCache = false;
        public SyntaxFilterEngine() : base()
        {
        }

        public override bool Contain(string input)
        {
            throw new NotImplementedException();
        }

        public override bool ContainIgnoreCase(string input)
        {
            if (_isVailCache)
            {
                MatchedWords.Clear();
                return Evaluate(input, PostFix);
            }
            return true;
        }

        public override bool IsVaild()
        {
            return _isVailCache && !string.IsNullOrEmpty(ComparableSource);
        }

        public override void UpdateComparableSource(string source)
        {
            lock (PostFix)
            {
                base.UpdateComparableSource(source);
                PartsSource = source.Split(new char[] { ' ' });
                _isVailCache = IsValid(PartsSource, ComparableSource);
                if (_isVailCache)
                {
                    PostFix = ConvertToPostFix(PartsSource);
                }
            }

        }

        private bool IsValid(string[] input, string rawInput)
        {
            foreach (var i in input)
            {
                if (string.IsNullOrEmpty(i))
                {
                    return false;
                }
            }

            Regex operators = new Regex(@"[&|]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            var len = input.Length;

            if (len == 0)
                return false;

            if (input.Select(c => c.Equals("(")).Count() != input.Select(c => c.Equals(")")).Count())
                return false;


            if (input[len - 1].Equals("&")
                | input[len - 1].Equals("|"))
                return false;

            string tempString = operators.Replace(rawInput, ".");
            string[] contains = new string[] { "( . )", "( )", ". .", ". )" };

            foreach (string s in contains)
            {
                if (tempString.Contains(s))
                    return false;
            }

            tempString = "";


            foreach (var st in input)
            {
                if (!IsOperatorAndBraket(st))
                {
                    tempString += "." + " ";
                }
                else
                {
                    tempString += st + " ";
                }
            }
            tempString = tempString.Remove(tempString.Length - 1);

            if (tempString.Contains(". ."))
                return false;

            if (input[0].Equals("&") || input[0].Equals("|"))
                return false;

            for (int i = 0; i < len - 1; i++)
            {
                if (IsOperator(input[i]) && IsOperator(input[i + 1]))
                {
                    return false;
                }
            }

            int begin = 0, end = 0;
            foreach (string st in input)
            {
                if (st.Equals("("))
                    begin++;
                if (st.Equals(")"))
                    end++;
                if (end > begin)
                    return false;
            }
            return true;
        }

        private bool IsOperatorAndBraket(string operate)
        {
            if (operate != null && operate.Length == 1
                && (operate.Equals("|")
                    | operate.Equals("&")
                    | operate.Equals("(")
                    | operate.Equals(")")))
                return true;

            return false;
        }

        private bool IsOperator(string operate)
        {
            if (operate != null && operate.Length == 1
                && (operate.Equals("|")
                    | operate.Equals("&")))
                return true;

            return false;
        }

        private string[] ConvertToPostFix(string[] inFix)
        {
            string[] postFix = new string[inFix.Length];
            string arrival;
            Stack<string> oprerator = new Stack<string>();//Creates a new Stack

            int i = 0;
            foreach (string st in inFix)//Iterates characters in inFix
            {
                if (!IsOperatorAndBraket(st))
                    postFix[i++] = st;
                else if (st.Equals("("))
                    oprerator.Push(st);
                else if (st.Equals(")"))//Removes all previous elements from Stack and puts them in 
                                        //front of PostFix.  
                {
                    arrival = oprerator.Pop();
                    while (!arrival.Equals("("))
                    {
                        postFix[i++] = arrival;
                        arrival = oprerator.Pop();
                    }
                }
                else
                {
                    if (oprerator.Count != 0 && Predecessor(oprerator.Peek(), st))//If find an operator
                    {
                        arrival = oprerator.Peek();
                        while (Predecessor(arrival, st))
                        {
                            postFix[i++] = arrival;
                            oprerator.Pop();

                            if (oprerator.Count == 0)
                                break;

                            arrival = oprerator.Peek();
                        }
                        oprerator.Push(st);
                    }
                    else
                        oprerator.Push(st);//If Stack is empty or the operator has precedence 
                }
            }
            while (oprerator.Count > 0)
            {
                arrival = oprerator.Pop();
                postFix[i++] = arrival;
            }
            return postFix;
        }

        private bool Predecessor(string firstOperator, string secondOperator)
        {
            string opString = "(|&";

            int firstPoint, secondPoint;

            int[] precedence = { 0, 12, 13 };// "(" has less prececence

            firstPoint = opString.IndexOf(firstOperator);
            secondPoint = opString.IndexOf(secondOperator);

            return (precedence[firstPoint] >= precedence[secondPoint]) ? true : false;
        }

        private bool Evaluate(string content, string[] postFix)
        {
            Stack<object> stack = new Stack<object>();
            int len = postFix.Length;
            bool result = false;
            for (int i = 0; i < len; i++)
            {
                if (postFix[i] == null)
                {
                    break;
                }
                if (!IsOperator(postFix[i]))
                {
                    stack.Push(postFix[i]);
                }
                else
                {
                    var a = stack.Pop();
                    var b = stack.Pop();
                    int type = 0;
                    bool a_bool = false;
                    bool b_bool = false;
                    string a_string = null;
                    string b_string = null;

                    if (a is Boolean && b is string)
                    {
                        type = 0;
                        a_bool = (bool)a;
                        b_string = (string)b;
                    }
                    else if (b is Boolean && a is string)
                    {
                        type = 1;
                        b_bool = (bool)b;
                        a_string = (string)a;
                    }
                    else if (a is bool && b is bool)
                    {
                        type = 2;
                        a_bool = (bool)a;
                        b_bool = (bool)b;
                    }
                    else
                    {
                        type = 3;
                        a_string = (string)a;
                        b_string = (string)b;
                    }

                    switch (type)
                    {
                        case 0:
                            if (postFix[i].Equals("&"))
                            {
                                result = AndOperateCalC(b_string, a_bool, content);
                            }
                            else
                            {
                                result = OrOperateCalC(b_string, a_bool, content);
                            }
                            break;
                        case 1:
                            if (postFix[i].Equals("&"))
                            {
                                result = AndOperateCalC(a_string, b_bool, content);
                            }
                            else
                            {
                                result = OrOperateCalC(a_string, b_bool, content);
                            }
                            break;
                        case 2:
                            if (postFix[i].Equals("&"))
                            {
                                result = a_bool && b_bool;
                            }
                            else
                            {
                                result = a_bool || b_bool;
                            }
                            break;
                        case 3:
                            if (postFix[i].Equals("&"))
                            {
                                result = AndOperateCalC(a_string, b_string, content);
                            }
                            else
                            {
                                result = OrOperateCalC(a_string, b_string, content);
                            }
                            break;
                    }

                    stack.Push(result);
                }

            }

            return result;
        }

        private bool OrOperateCalC(string A, string B, string Cmp)
        {
            int res = 0;
            int startA = Cmp.IndexOf(A, StringComparison.InvariantCultureIgnoreCase);
            int startB = Cmp.IndexOf(B, StringComparison.InvariantCultureIgnoreCase);
            if (startA != -1)
            {
                res = 1;
                MatchedWords.Add(new MatchedWord(startA, A, Cmp));
            }
            if (startB != -1)
            {
                res = 2;
                MatchedWords.Add(new MatchedWord(startB, B, Cmp));
            }

            return res != 0;
        }

        private bool AndOperateCalC(string A, string B, string Cmp)
        {
            int startA = Cmp.IndexOf(A, StringComparison.InvariantCultureIgnoreCase);
            int startB = Cmp.IndexOf(B, StringComparison.InvariantCultureIgnoreCase);
            if (startA != -1 && startB != -1)
            {
                MatchedWords.Add(new MatchedWord(startA, A, Cmp));
                MatchedWords.Add(new MatchedWord(startB, B, Cmp));
                return true;
            }
            return false;
        }

        private bool OrOperateCalC(string A, bool B, string Cmp)
        {
            int res = 0;
            int startA = Cmp.IndexOf(A, StringComparison.InvariantCultureIgnoreCase);

            if (startA != -1)
            {
                res = 1;
                MatchedWords.Add(new MatchedWord(startA, A, Cmp));
            }
            if (B)
            {
                res = 2;
            }

            return res != 0;
        }

        private bool AndOperateCalC(string A, bool B, string Cmp)
        {
            int startA = Cmp.IndexOf(A, StringComparison.InvariantCultureIgnoreCase);

            if (startA != -1 && B)
            {
                MatchedWords.Add(new MatchedWord(startA, A, Cmp));
                return true;
            }
            return false;
        }
    }
}
