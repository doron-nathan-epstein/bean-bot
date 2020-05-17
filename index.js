require("dotenv").config();
const Discord = require("discord.js");
const fs = require("fs");

const client = new Discord.Client();
client.commands = new Discord.Collection();

fs.readdir("./events/", (err, files) => {
  files.forEach((file) => {
    const eventHandler = require(`./events/${file}`);
    const eventName = file.split(".")[0];
    client.on(eventName, async (...args) => eventHandler(client, ...args));
  });
});

fs.readdir("./commands/", (err, files) => {
  files.forEach((file) => {
    const command = require(`./commands/${file}`);
    client.commands.set(command.name, command);
  });
});

console.log("BEAN BOT STARTUP");
console.log("================");
console.log("");
console.log("Logging into Discord...")
client.login(process.env.BOT_TOKEN);
