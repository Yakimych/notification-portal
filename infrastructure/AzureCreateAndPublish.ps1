param (
  [string]$ADMIN_USERNAME,
  [string]$ADMIN_PASSWORD
)

if (-not($ADMIN_USERNAME)) { Throw "ADMIN_USERNAME should be specified" }
if (-not($ADMIN_PASSWORD)) { Throw "ADMIN_PASSWORD should be specified" }

$PROJECT_FILE = "../src/NotificationPortal.Web/NotificationPortal.Web.csproj"
$OUTPUT_FOLDER = "./publish_output"

rm -rf $OUTPUT_FOLDER
dotnet publish $PROJECT_FILE -c release -o $OUTPUT_FOLDER
dotnet run -- $OUTPUT_FOLDER $ADMIN_USERNAME $ADMIN_PASSWORD
