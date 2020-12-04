const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const frontendDirName = "frontend";
const getFileInFrontendDir = (fileName) => `./${frontendDirName}/${fileName}`;

module.exports = {
  entry: {
    index: getFileInFrontendDir("index.ts"),
    challenges: getFileInFrontendDir("challenges.ts"),
    component1: getFileInFrontendDir("component1.ts"),
    component2: getFileInFrontendDir("component2.ts"),
  },
  output: {
    path: path.resolve(__dirname, "wwwroot/transpiled"),
    filename: `[name].js`,
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
      filename: `css/[name].css`,
    }),
  ],
};
