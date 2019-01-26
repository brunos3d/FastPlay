using System;

namespace FastPlay {
	public interface IValueConverter {

		bool CanConvert(Type from, Type to);
	}

	public interface IValueConverter<T> : IValueConverter {

		T Convert(object value);
	}
}
