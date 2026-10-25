#!/usr/bin/env python3
import sys
from datetime import datetime, timezone
from colorama import Fore, Style, init

init(autoreset=True)

try:
    import browser_cookie3
except ImportError:
    print("Install browser-cookie3: python -m pip install browser-cookie3", file=sys.stderr)
    sys.exit(1)

def get_vinted_cookies():
    return [
        cookie for cookie in browser_cookie3.firefox(domain_name="vinted.fr")
        if (
            cookie.domain.lstrip(".").lower() == "vinted.fr"
            or cookie.domain.lower().endswith(".vinted.fr")
        )
    ]

cookies = get_vinted_cookies()
required_names = {"access_token_web", "refresh_token_web"}
found_names = {cookie.name for cookie in cookies}

if not required_names.issubset(found_names):
    missing_names = ", ".join(sorted(required_names - found_names))
    print(f"Cookie(s) missing: {missing_names}")
    print("Connectez-vous à Vinted dans Firefox, puis réessayez...")
    sys.exit(1)

for name in ("access_token_web", "refresh_token_web"):
    matches = [
        cookie for cookie in cookies
        if cookie.name == name
    ]
    
    for cookie in matches:
        try:
            expiration = datetime.fromtimestamp(cookie.expires, timezone.utc).isoformat()
        except (OSError, OverflowError, TypeError, ValueError):
            expiration = "session cookie or unknown expiration"
        print(f"  {cookie.domain}; {cookie.name} : {Fore.LIGHTMAGENTA_EX}{cookie.value}{Style.RESET_ALL} expiration (UTC): {expiration}")
