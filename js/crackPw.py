import itertools

# Passwords for the four users
passwords = [
    "Dummy@X123#",
    "Dummy@Y123#",
    "Dummy@W123#",
    "Dummy@12345#"
]

# Common character substitutions
substitutions = {
    'a': '@',
    'A': '@',
    'e': '3',
    'E': '3',
    'i': '1',
    'I': '1',
    'o': '0',
    'O': '0',
    's': '$',
    'S': '$',
    't': '7',
    'T': '7'
}

wordlist = set()

# Generate variations
for password in passwords:
    # Add original password
    wordlist.add(password)

    # Add lowercase and uppercase variations
    wordlist.add(password.lower())
    wordlist.add(password.upper())

    # Add variations with common substitutions
    for char in password:
        if char in substitutions:
            for variant in itertools.product([char, substitutions[char]], repeat=len(password)):
                wordlist.add(''.join(variant))

# Save the wordlist to a file
with open('wordlist.txt', 'w') as f:
    f.write('\n'.join(wordlist))

print("Wordlist generated with", len(wordlist), "entries.")