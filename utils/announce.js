module.exports = function (connection, tts, disconnect) {
  let gtts =
    "https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=" +
    encodeURI(tts);

  let dispatcher = connection.play(gtts, {
    volume: 1,
  });

  dispatcher.on("finish", () => {
    dispatcher.destroy();

    if (disconnect) {
      connection.disconnect();
    }
  });
};
