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
client.on("voiceStateUpdate", (oState, nState) => {
  let newUserChannel = nState.voiceChannel
  let oldUserChannel = oState.voiceChannel

  if(newUserChannel != oldUserChannel) {
    if(oldUserChannel === undefined && newUserChannel !== undefined) {
      console.log(nState.member.displayName + " has joined the channel.")
      // User Joins a voice channel

    } else if(newUserChannel === undefined){

      console.log(nState.member.displayName + " has left the channel.")

    }
  }
})
client.login(process.env.BOT_TOKEN)