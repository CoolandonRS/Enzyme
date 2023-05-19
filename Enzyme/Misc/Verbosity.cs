namespace Enzyme.Misc; 

public static class Verbosity {
    public static bool showErrors() {
        return get() >= 1;
    }
    
    public static bool showErrorData() {
        return get() >= 2;
    }

    public static bool showDebug() {
        return get() >= 3;
    }

    public static void WriteError(string msg) {
        if (showErrors()) Console.WriteLine(msg);
    }

    public static void WriteErrorData(string msg) {
        if (showErrorData()) Console.WriteLine(msg);
    }

    public static void WriteDebug(string msg) {
        if (showDebug()) Console.WriteLine(msg);
    }

    private static int get() {
        return Program.argHandler.GetValue("verbosity").AsInt();
    }
}