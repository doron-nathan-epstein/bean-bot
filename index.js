require("dotenv").config()
const Discord = require("discord.js")
const client = new Discord.Client()
const PREFIX = '!mb'

client.on("ready", () => {
  console.log(`Logged in as ${client.user.tag}!`)
})

client.on("message", async message => {
  if(message.author.bot) {
    return;
  }

  if(!message.guild) {
    return;
  }

  if(message.content.indexOf(PREFIX) !== 0) {
    return;
  }

  if (message.content === '!mb join') {
    if (message.member.voice.channel) {
      await message.member.voice.channel.join();
    } else {
      message.reply('You need to join a voice channel first!');
    }
  }

  if (message.content === '!mb leave') {
    let connection = client.voice.connections.get(message.guild.id)
    if(connection !== undefined) {
      announce(connection, "Mr Bean bids you farewell", true)
    }
  }
})

client.on("voiceStateUpdate", async (oldState, newState) => {
  if(oldState.member.user.bot) {
    return
  }

  if(oldState !== null) {
    let connection = client.voice.connections.get(oldState.guild.id)
    if(connection !== undefined) {
      if(connection.voice.channelID === oldState.channelID) {
        announce(connection, oldState.member.displayName + "has left the channel", false)
      }
    }
  }

  if(newState !== null) {
    let connection = client.voice.connections.get(newState.guild.id)
    if(connection !== undefined) {
      if(connection.voice.channelID === newState.channelID) {
        announce(connection, oldState.member.displayName + "has joined the channel", false)
      }
    }
  }
})

function announce(connection, tts, disconnect) {
  let gtts = "https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=" + encodeURI(tts)

  let dispatcher = connection.play(gtts, {
    volume: 1,
  });

  dispatcher.on("finish", () => {
    dispatcher.destroy();

    if(disconnect) {
      connection.disconnect();
    }
  });
}

client.login(process.env.BOT_TOKEN)