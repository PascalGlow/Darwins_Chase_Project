import json

with open('Cards.json', 'r') as file:
	data = json.load(file)

sum = 0
for element in data["Umwelt_Karten"]:
	sum += len(element["Frage"].split(' '))
print(sum/len(data["Umwelt_Karten"]))
