using System.Text;

//json["hoge"]["fuga"][2]["nyan"]
//json.get("hoge").get("fuga").get(2).get("nyan")

class Parser{
    private object? _JsonObject;

    public Parser(string json){
        char[] json2chars = json.ToCharArray();

        int ind = 0;
        _JsonObject = ParseJson(ref ind, json2chars);
    }

    public override string ToString(){
        return ToString(_JsonObject);
    }

    private string ToString(object? target){
        string ret = "";

        if(target is Dictionary<string, object> tDic){
            ret = "{" + Dic2Str(tDic) + "}";
        }else if(target is List<object> tList){
            ret = "[" + List2Str(tList) + "]";
        }else if(target is null){
            ret = "(null)";
        }else if(target is string){
            ret = "(" + target.GetType().ToString().Replace("System.", "") + ")\"" + target.ToString() + "\"";
        }else{
            ret = "(" + target.GetType().ToString().Replace("System.", "") + ")" + target.ToString();
        }

        return ret;
    }

    public string Dic2Str(Dictionary<string, object> dic){
        StringBuilder sb = new StringBuilder();

        bool first = true;
        foreach(var (key, value) in dic){
            if(first){
                first = false;
            }else{
                sb.Append(", ");
            }

            sb.Append("\"" + key + "\": " + ToString(value));
        }

        return sb.ToString();
    }

    public string List2Str(List<object> list){
        StringBuilder sb = new StringBuilder();

        bool first = true;
        foreach(object obj in list){
            if(first){
                first = false;
            }else{
                sb.Append(", ");
            }

            sb.Append(ToString(obj));
        }

        return sb.ToString();
    }

    // 1, 2, 3, 4, 5

    private Dictionary<string, object?> ParseObject(ref int ind, char[] json2chars){
        Dictionary<string, object?> ret = new Dictionary<string, object?>();

        ind++;
        if(json2chars[ind] == '}'){
            ind++;
            return ret;
        }

        while(true){

            string key = GetKey(ref ind, json2chars);

            SkipInvalidChar(ref ind, json2chars);

            // if(json2chars[ind] != ':'){
            //     throw Error;
            // }
            ind++;

            object? value = ParseJson(ref ind, json2chars);

            ret.Add(key, value);

            SkipInvalidChar(ref ind, json2chars);
            
            if(json2chars[ind] == '}'){
                break;
            }
            // if(json2chars[ind] != ','){
            //     throw Error;
            // }

            ind++;
        }
        ind++;

        return ret;
    }

    private List<object?> ParseArray(ref int ind, char[] json2chars){
        List<object?> ret = new List<object?>();

        ind++;
        SkipInvalidChar(ref ind, json2chars);
        if(json2chars[ind] == ']'){
            ind++;
            return ret;
        }
        while(true){

            object value = ParseJson(ref ind, json2chars);

            ret.Add(value);

            SkipInvalidChar(ref ind, json2chars);

            if(json2chars[ind] == ']'){
                break;
            }
            // if(json2chars[ind] != ','){
            //     throw Errow;
            // }

            ind++;
        }
        ind++;

        return ret;
    }

    private double ParseNumber(ref int ind, char[] json2chars){
        double ret = 0;
        bool minus = false;
        if(json2chars[ind] == '-'){
            minus = true;
            ind++;
        }

        while(ind < json2chars.Length && Char.IsDigit(json2chars[ind])){

            ret = ret*10 + (int)(json2chars[ind]-'0');
            ind++;
        }

        if(ind+1 < json2chars.Length && json2chars[ind] == '.'){
            ind++;

            double digit = 0.1;
            while(ind < json2chars.Length && Char.IsDigit(json2chars[ind])){
                ret += (int)(json2chars[ind]-'0') * digit;

                digit *= 0.1;
                ind++;
            }

        }

        if(ind+1 < json2chars.Length && json2chars[ind] == 'e'){
            ind++;

            bool ism = false;
            if(json2chars[ind] == '-'){
                ism = true;
                ind++;
            }

            int indexPart = 0;
            while(ind < json2chars.Length && Char.IsDigit(json2chars[ind])){
                indexPart = indexPart*10 + (int)(json2chars[ind]-'0');
                ind++;
            }

            double v = (ism) ? 0.1 : 10;
            for(int i = 0; i < indexPart; i++){
                ret *= v;
            }
        }

        if(minus){
            ret *= -1;
        }

        return ret;
    }

    private string ParseString(ref int ind, char[] json2chars){
        StringBuilder sb = new StringBuilder();

        ind++;
        while(true){
            // if(ind >= json2chars.Length){
            //     throw Error;
            // }

            if(json2chars[ind] == '"'){
                break;
            }
            sb.Append(json2chars[ind]);
            ind++;
        }
        ind++;

        return sb.ToString();
    }

    private bool ParseBool(ref int ind, char[] json2chars){
        bool ret;

        if(json2chars[ind] == 't'){
            ind += 4;
            ret = true;
        }else{
            ind += 5;
            ret = false;
        }

        return ret;
    }

    private object? ParseNull(ref int ind, char[] json2chars){
        ind += 4;

        return null;
    }


    private char[] _invalidChars = " 　\t\n".ToCharArray();
    private void SkipInvalidChar(ref int ind, char[] json2chars){
        while(ind < json2chars.Length){
            if(!_invalidChars.Contains(json2chars[ind])){
                break;
            }

            ind++;
        }
    }

    private object? ParseJson(ref int ind, char[] json2chars){
        object? ret;

        SkipInvalidChar(ref ind, json2chars);

        if(json2chars[ind] == '\"'){
            ret = ParseString(ref ind, json2chars);
        }else if(Char.IsDigit(json2chars[ind]) || json2chars[ind] == '-'){
            ret = ParseNumber(ref ind, json2chars);
        }else if(json2chars[ind] == 'n'){
            ret = ParseNull(ref ind, json2chars);
        }else if(json2chars[ind] == 't' || json2chars[ind] == 'f'){
            ret = ParseBool(ref ind, json2chars);
        }else if(json2chars[ind] == '{'){
            ret = ParseObject(ref ind, json2chars);
        }else{
            ret = ParseArray(ref ind, json2chars);
        }

        return ret;
    }

    private string GetKey(ref int ind, char[] json2chars){
        SkipInvalidChar(ref ind, json2chars);
        // if(json2chars[ind] != '"'){
        //     throw Error;
        // }
        ind++;

        StringBuilder sb = new StringBuilder();
        while(true){
            // if(ind >= json2chars.Length){
            //     throw Error;
            // }

            if(json2chars[ind] == '"'){
                break;
            }

            sb.Append(json2chars[ind]);
            ind++;
        }

        ind++;
        return sb.ToString();
    }


}

class MainClass{
    public static void Main(){
        var sw = new System.Diagnostics.Stopwatch();


        StringBuilder sb = new StringBuilder();

        string input = "{\"a\":8}";


        sw.Start();
        
        Parser prs = new Parser(input);
        
        sw.Stop();
        
        Console.WriteLine(input);
        Console.WriteLine(prs.ToString());
        Console.WriteLine($"{sw.Elapsed}");


    }
}
