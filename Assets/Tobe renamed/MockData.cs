using System.Collections;
using System.Collections.Generic;

public class MockData {
	//public static List<Item> items = new List<Item>();

	public static List<Item> GetMockData() {
		List<Item> items = new List<Item>();
		int count = 20;
		for (int i = 0; i < count; i++) {
			Item item = new Item();
			item.title = "Mock Data " + i;
			items.Add(item);
		}
		return items;
	}

//	public static void AddToMock(int amount) {
//		for (int i = items.Count; i < items.Count + amount; i++) {
//			Item item = new Item();
//			item.title = "Mock Data " + i;
//			items.Add(item);
//		}
//	}
}
