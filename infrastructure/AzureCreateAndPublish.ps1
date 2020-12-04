param (
  [string]$ADMIN_USERNAME,
  [string]$ADMIN_PASSWORD
)

if (-not($ADMIN_USERNAME)) { Throw "ADMIN_USERNAME should be specified" }
if (-not($ADMIN_PASSWORD)) { Throw "ADMIN_PASSWORD should be specified" }

$PROJECT_DIR = "../src/NotificationPortal.Web/"
$PROJECT_FILE = "NotificationPortal.Web.csproj"
$FULL_PROJECT_PATH = Join-Path -Path $PROJECT_DIR -ChildPath $PROJECT_FILE

$OUTPUT_FOLDER = "./publish_output"

rm -rf $OUTPUT_FOLDER

Set-Location $PROJECT_DIR
npm install
npm run release
Set-Location ../../infrastructure

dotnet publish $FULL_PROJECT_PATH -c release -o $OUTPUT_FOLDER
dotnet run -- $OUTPUT_FOLDER $ADMIN_USERNAME $ADMIN_PASSWORD
