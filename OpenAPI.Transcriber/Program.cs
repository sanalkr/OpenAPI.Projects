// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
{
    Console.WriteLine("Enter the file path to transcribe");
    return;
}

string path = args[0];

string destination = Path.GetDirectoryName(path);
string filename = Path.GetFileNameWithoutExtension(path);

using Stream audioStream = File.OpenRead(path);

try
{
    OpenAIClient client = new OpenAIClient(configuration["OpenAIKey"]);

    var transcriptionOptions = new AudioTranscriptionOptions()
    {
        AudioData = BinaryData.FromStream(audioStream),
        ResponseFormat = AudioTranscriptionFormat.Srt        
    };

    Response<AudioTranscription> transcriptionResponse = await client.GetAudioTranscriptionAsync(
        deploymentId: "whisper-1", 
        transcriptionOptions);

    AudioTranscription transcription = transcriptionResponse.Value;

    Console.Write(transcription.Text);

    File.WriteAllText($"{destination}\\{filename}.srt", transcription.Text);
}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex.Message}");
}
