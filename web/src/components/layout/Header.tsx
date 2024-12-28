import Image from "next/image";

export default function Header() {
    return (
        <header className="fixed top-0 w-full inset-x-0 bg-white z-50 shadow-sm">
            <nav className="container flex items-center justify-between py-4">
                <Image src="/images/logo.svg" alt="Azure Task" width={120} height={40} />

                <ul className="flex items-center space-x-4">
                    <li className="btn btn-outline">Login</li>
                    <li className="btn btn-primary">Get Started</li>
                </ul>
            </nav>
        </header>
    )
}