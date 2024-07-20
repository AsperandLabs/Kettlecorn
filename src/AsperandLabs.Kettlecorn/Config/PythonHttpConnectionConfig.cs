namespace AsperandLabs.Kettlecorn.Config;

public class PythonHttpConnectionConfig
{
    //Set this to the same dir as your python venv
    public string WorkingDirectory { get; set; }
    //Set this to the path to the uvicorn/fastapi/gunicorn binary in your venv (run which uvicorn to get path from python project)
    public string RelativeStartFile { get; set; }
    //This is the same entry point that you would pass to uvicorn
    public string EntryPoint { get; set; }
}