using TL;

var photoDirectory = Path.Combine(Environment.CurrentDirectory, "photos");
if (!Directory.Exists(photoDirectory))
    Directory.CreateDirectory(photoDirectory);

var docDirectory = Path.Combine(Environment.CurrentDirectory, "documents");
if (!Directory.Exists(docDirectory))
    Directory.CreateDirectory(docDirectory);


using var client = new WTelegram.Client(Config);
await client.LoginUserIfNeeded();

client.OnUpdate += UpdateHandler;

async Task UpdateHandler(UpdatesBase updatesBase)
{
    foreach (Update? update in updatesBase.UpdateList)
    {
        if (update is not UpdateNewMessage newMessage)
            continue;

        var fromId = newMessage.message.Peer.ID;
        Console.Write($"msg from {fromId} at {newMessage.message.Date}");

        if (newMessage.message is not Message msg)
            continue;

        Console.WriteLine($" text: {msg.message}");

        if (msg.media is MessageMediaPhoto messageMediaPhoto)
        {
            if (messageMediaPhoto.photo is Photo photo) { }
        }
        if (msg.media is MessageMediaPhoto { photo: Photo thePicture })
        {
            var path = Path.Combine(photoDirectory, $"{thePicture.id}.jpg");
            await using var fs = new FileStream(path, FileMode.OpenOrCreate);
            await client.DownloadFileAsync(thePicture, fs);
        }

        else if (msg.media is MessageMediaDocument { document: Document doc })
        {
            var filename = doc.Filename ?? $"{doc.id}.{doc.mime_type.Split('/')[1]}";
            var path = Path.Combine(docDirectory, filename);
            await using var fs = new FileStream(path, FileMode.OpenOrCreate);
            await client.DownloadFileAsync(doc, fs);
        }
    }
}

while (true)
    await Task.Delay(TimeSpan.FromSeconds(1));



//alternative

/*client.RegisterUpdateType<UpdateNewMessage>(OnNewMessage);

async Task OnNewMessage(UpdateNewMessage message, UpdatesBase? updateBase)
{
    Console.WriteLine($"msg from {message.message.Peer.ID}");
}*/

static string? Config(string what)
{
    switch (what)
    {
        case "session_pathname": return Path.Join(Environment.CurrentDirectory, "mysessionFILE.session");
        case "api_id":           return "YOUR_API_ID";
        case "api_hash":         return "YOUR_API_HASH";
        case "phone_number":     return "+12025550156";
        case "verification_code":
            Console.Write("Code: ");
            return Console.ReadLine()!;
        case "first_name": return "John";      // if sign-up is required
        case "last_name":  return "Doe";       // if sign-up is required
        case "password":   return "secret!"; // if user has enabled 2FA
        default:           return null;
    }
}