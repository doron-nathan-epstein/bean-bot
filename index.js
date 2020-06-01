require("dotenv").config();
const discord = require("discord.js");
const fs = require("fs");
const context = require("./repository/context.js");

console.log("================");
console.log("BEAN BOT STARTUP");
console.log("================");
console.log("");

const db_context = new context();

console.log("Creating Discord client...");
const client = new discord.Client();

console.log("\tSetting up bot events...");
client.commands = new discord.Collection();
fs.readdir("./events/", (err, files) => {
  files.forEach((file) => {
    const eventHandler = require(`./events/${file}`);
    const eventName = file.split(".")[0];
    client.on(eventName, async (...args) =>
      eventHandler(client, db_context, ...args)
    );
  });
});

console.log("\tSetting up bot commands...");
fs.readdir("./commands/", (err, files) => {
  files.forEach((file) => {
    const command = require(`./commands/${file}`);
    client.commands.set(command.name, command);
  });
});
console.log("");

console.log("Logging into Discord...");
client.login(process.env.BOT_TOKEN);

process.on("exit", () => shutdown());
process.on("SIGHUP", () => process.exit(128 + 1));
process.on("SIGINT", () => process.exit(128 + 2));
process.on("SIGTERM", () => process.exit(128 + 15));

function shutdown() {
  console.log("");
  console.log("=================");
  console.log("BEAN BOT SHUTDOWN");
  console.log("=================");
  console.log("");

  ctx.close();

  console.log("Closing Discord Client connection...");
  client.destroy();
  console.log("Discord Client connection closed.");
}
