require("dotenv").config();
const discord = require("discord.js");
const fs = require("fs");

start()
  .then((client) => {
    console.log("\tSetting up bot events...");
    client.commands = new discord.Collection();
    fs.readdir("./events/", (err, files) => {
      files.forEach((file) => {
        const eventHandler = require(`./events/${file}`);
        const eventName = file.split(".")[0];
        const app = {
          client: client
        };

        client.on(eventName, async (...args) => eventHandler(app, ...args));
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

function start() {
  console.log("================");
  console.log("BEAN BOT STARTUP");
  console.log("================");
  console.log("");

  console.log("Creating Discord client...");
  return new discord.Client();
}

function shutdown(client) {
  console.log("");
  console.log("=================");
  console.log("BEAN BOT SHUTDOWN");
  console.log("=================");
  console.log("");

  console.log("Closing Discord Client connection...");
  client.destroy();
  console.log("Discord Client connection closed.");
}
