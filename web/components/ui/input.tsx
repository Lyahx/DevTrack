import * as React from "react"
import { Input as InputPrimitive } from "@base-ui/react/input"

import { cn } from "@/lib/utils"

function Input({ className, type, ...props }: React.ComponentProps<"input">) {
  return (
    <InputPrimitive
      type={type}
      data-slot="input"
      className={cn(
        "h-8 w-full min-w-0 rounded-md border border-border bg-input px-3 py-1 text-[13px] text-text outline-none transition-colors placeholder:text-text-faint focus-visible:border-border-strong disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-warning",
        className
      )}
      {...props}
    />
  )
}

export { Input }
