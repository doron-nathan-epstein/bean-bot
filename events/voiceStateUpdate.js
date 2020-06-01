const announce = require("../utils/announce.js");

module.exports = async (client, db_context, oldState, newState) => {
  if (oldState.member.user.bot) {
    return;
  }

  if (oldState.channelID === newState.channelID) {
    return;
  }

  if (oldState !== null) {
    const connection = client.voice.connections.get(oldState.guild.id);
    if (connection !== undefined) {
      if (connection.voice.channelID === oldState.channelID) {
        announce(
          connection,
          oldState.member.displayName + "has left the channel",
          false
        );
      }
    }
  }

  if (newState !== null) {
    const connection = client.voice.connections.get(newState.guild.id);
    if (connection !== undefined) {
      if (connection.voice.channelID === newState.channelID) {
        announce(
          connection,
          oldState.member.displayName + "has joined the channel",
          false
        );
      }
    }
  }
};
