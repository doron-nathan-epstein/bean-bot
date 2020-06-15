const announce = require("../utils/announce.js");

module.exports = {
  name: "leave",
  description: "Ask me to leave the current voice channel I am connected to.",
  guildOnly: true,
  async execute(message, appDAO, args) {
    const connection = message.client.voice.connections.get(message.guild.id);
    if (connection !== undefined) {
      announce(connection, "Mr Bean bids you farewell", true);
    }
  },
};
