import { IconSearch, IconChevronDown } from "@tabler/icons-react";

export default function Menu() {
  return (
    <section role="menu" className="w-full flex justify-between space-x-4">
      <div className="flex space-x-2 items-center w-1/4 p-2 border rounded-lg">
        <IconSearch size={20} stroke={1.5} />
        <input
          type="text"
          placeholder="Search tasks"
          className="border-none w-full bg-transparent outline-none"
        />
      </div>

      <ul className="flex space-x-2 items-center">
        <li>
          <button className="flex space-x-1 items-center p-2">
            <span>Filter By</span>
            <IconChevronDown size={20} stroke={1.5} />
          </button>
        </li>

        <li>
          <button className="flex space-x-1 items-center p-2">
            <span>Sort By</span>
            <IconChevronDown size={20} stroke={1.5} />
          </button>
        </li>
      </ul>
    </section>
  );
}
