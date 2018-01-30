using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateClient2.Views {
    public class Presenter<T> {
        protected readonly T _view;

        public Presenter(T view) {
            _view = view;
        }
    }
}