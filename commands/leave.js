const announce = require("../utils/announce.js");

module.exports = {
  name: "leave",
  description: "Ask me to leave the current voice channel I am connected to.",
  async execute(message, args) {
    let connection = message.client.voice.connections.get(message.guild.id);
    if (connection !== undefined) {
      announce(connection, "Mr Bean bids you farewell", true);
    }
  },
};
