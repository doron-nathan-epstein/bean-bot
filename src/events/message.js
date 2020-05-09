const announce = require("../utils/announce.js");
const PREFIX = "!mb";

module.exports = async (client, message) => {
  if (message.author.bot) {
    return;
  }

  if (!message.guild) {
    return;
  }

  if (!message.content.startsWith(PREFIX)) {
    return;
  }

  if (message.content === "!mb join") {
    if (message.member.voice.channel) {
      await message.member.voice.channel.join();
    } else {
      message.reply("You need to join a voice channel first!");
    }
  }

  if (message.content === "!mb leave") {
    let connection = client.voice.connections.get(message.guild.id);
    if (connection !== undefined) {
      announce(connection, "Mr Bean bids you farewell", true);
    }
  }
};
