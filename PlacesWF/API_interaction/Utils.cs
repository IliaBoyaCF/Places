using System.Text.Json.Nodes;

namespace PlacesWF.API_interaction;

internal class Utils
{
    public static T FromTask<T>(Task<JsonObject> task, Func<JsonObject, T> jsonParser)
    {
        task.Wait();
        return jsonParser(task.Result);
    }

    public static T WaitTask<T>(Task<T> task)
    {
        task.Wait();
        return task.Result;
    }
}
