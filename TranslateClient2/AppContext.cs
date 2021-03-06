﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GlobalHotKey;
using TranslateClient2.Translator;

namespace TranslateClient2 {
    internal class AppContext {
        public static AppContext Instance { get; } = new AppContext();

        private AppContext() { }

        public ITranslator Translator { get; } = new YandexTranslator();

        public HotKeyManager HotKeyManager { get; } = new HotKeyManager();

        ~AppContext() {
            HotKeyManager.Dispose();
        }
    }
}