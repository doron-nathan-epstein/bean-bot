require("dotenv").config()
const Discord = require("discord.js")
const client = new Discord.Client()

client.on("ready", () => {
  console.log(`Logged in as ${client.user.tag}!`)
})

client.on("message", msg => {
  if (msg.content === "!mb ping") {
    msg.reply("Pong! by Bean")
  }
})

client.on("voiceStateUpdate", async (oldState, newState) => {
  if(oldState.channelID !== newState.channelID) {
    if(newState.channel !== null) {
      let connection = await newState.channel.join();
      //connection.play();
      console.log(newState.channel.name + " - " + newState.member.displayName + " has joined the channel.")
    }
    if(oldState.channel !== null) {
      console.log(oldState.channel.name + " - " + oldState.member.displayName + " has left the channel.")
    }
  }
})

client.login(process.env.BOT_TOKEN)