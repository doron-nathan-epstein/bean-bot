require("dotenv").config();
const discord = require("discord.js");
const fs = require("fs");
const context = require("./repository/context.js");
const blizzardAuth = require("./utils/blizzardAuth.js");

console.log("================");
console.log("BEAN BOT STARTUP");
console.log("================");
console.log("");

const db_context = new context();

new blizzardAuth()
  .getToken()
  .then((token) => db_context.setupDatabase(token))
  .then(() => {
    console.log("Creating Discord client...");
    return new discord.Client();
  })
  .then((client) => {
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

    return client;
  })
  .then((client) => {
    console.log("\tSetting up bot commands...");
    fs.readdir("./commands/", (err, files) => {
      files.forEach((file) => {
        const command = require(`./commands/${file}`);
        client.commands.set(command.name, command);
      });
    });
    console.log("");

    return client;
  })
  .then((client) => {
    console.log("Logging into Discord...");
    client.login(process.env.BOT_TOKEN);

    return client;
  })
  .then((client) => {
    process.on("exit", () => shutdown(client));
    process.on("SIGHUP", () => process.exit(128 + 1));
    process.on("SIGINT", () => process.exit(128 + 2));
    process.on("SIGTERM", () => process.exit(128 + 15));
  });

function shutdown(client) {
  console.log("");
  console.log("=================");
  console.log("BEAN BOT SHUTDOWN");
  console.log("=================");
  console.log("");

  this.db_context.close();

  console.log("Closing Discord Client connection...");
  client.destroy();
  console.log("Discord Client connection closed.");
}
