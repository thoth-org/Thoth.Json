import numbers
import json

print(isinstance(False, (int, float, complex)))

print(isinstance(False, bool))

# print(null is None)

print(json.loads("null"))
print(json.dumps(None))

v = {'name': None, 'age': 25}
print(v)
print(v['name'])
# print(v['name22'])
print("name" in v)
print("name22" in v)
