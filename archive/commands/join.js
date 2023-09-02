module.exports = {
  name: "join",
  description:
    "Ask me to join the voice channel you are currently connected to.",
  guildOnly: true,
  async execute(app, message, args) {
    if (message.member.voice.channel) {
      await message.member.voice.channel.join();
    } else {
      message.reply("You need to join a voice channel first!");
    }
  },
};
