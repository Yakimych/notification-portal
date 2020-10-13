$PROJECT_FILE = "../src/NotificationPortal.Web/NotificationPortal.Web.csproj"
$OUTPUT_FOLDER = "./publish_output"

rm -rf $OUTPUT_FOLDER
dotnet publish $PROJECT_FILE -c release -o $OUTPUT_FOLDER
dotnet run -- $OUTPUT_FOLDER
