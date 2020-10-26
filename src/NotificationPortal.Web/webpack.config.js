const path = require("path");
// TODO: Remove
const HtmlWebpackPlugin = require("html-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

// TODO: Is it possuble to use Microsoft.TypeScript.MSBuild for development-mode instead?
module.exports = (env, argv) => ({
  entry: "./src/index.ts",
  output: {
    path: path.resolve(__dirname, "wwwroot/transpiled"),
    filename: `[name]${argv.mode === "development" ? "" : ".[chunkhash]"}.js`,
    publicPath: "/",
  },
  resolve: {
    extensions: [".js", ".ts"],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: "ts-loader",
      },
      {
        test: /\.css$/,
        use: [MiniCssExtractPlugin.loader, "css-loader"],
      },
    ],
  },
  plugins: [
    new CleanWebpackPlugin(),
    new MiniCssExtractPlugin({
      filename: "css/[name].[chunkhash].css",
    }),
  ],
});
